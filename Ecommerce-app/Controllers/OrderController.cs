using Ecommerce_app.Areas.Identity.Models;
using Ecommerce_app.Data;
using Ecommerce_app.Helpers;
using Ecommerce_app.Models.Orders;
using Ecommerce_app.Models.Products;
using Ecommerce_app.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Order = Ecommerce_app.Models.Orders.Order;

namespace Ecommerce_app.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly EcommerceAppContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private IConfiguration _config;

        public OrderController(EcommerceAppContext context, UserManager<AppUser> userManager, IConnectionMultiplexer multiplexer, IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _connectionMultiplexer = multiplexer;
            _config = config;
        }

        #region 個人訂單
        /// <summary>
        /// 取得使用者的訂單列表
        /// </summary>
        /// <returns>使用者的訂單列表</returns>
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            // 取得目前使用者
            // var user = await _userManager.GetUserAsync(User);
            var userId = _userManager.GetUserId(User);

            // 取得使用者的訂單列表，包括訂單項目
            var orders = await _context.Order
                .Where(x => x.UserId == userId)
                .Include(e => e.OrderItems)
                .ToListAsync();

            // 回傳訂單列表
            return View(orders);
        }

        /// <summary>
        /// 取得訂單詳細資料
        /// </summary>
        /// <param name="orderId">訂單編號</param>
        /// <returns>訂單詳細資料視圖</returns>
        public async Task<IActionResult> Details(string orderId)
        {
            // 取得訂單資料，根據訂單編號查詢
            var order = await _context.Order
                .Include(e => e.OrderItems) 
                .FirstOrDefaultAsync(x => x.OrderId == orderId);

            // 返回訂單詳細資料視圖
            return View(order);
        }
        #endregion

        #region 結帳流程
        /// <summary>
        /// 處理結帳邏輯
        /// </summary>
        /// <returns>結帳視圖模型</returns>
        public async Task<IActionResult> Checkout()
        {

            var orderId = Guid.NewGuid().ToString("N").Substring(0, 16);    // 產生訂單編號(16碼)
            var user = await _userManager.GetUserAsync(User); // 取得目前使用者
            var orderItems = GetOrderItems(user!.Id);         // 取得使用者的訂單項目
            var totalAmount = GetTotalAmount(orderItems);     // 取得訂單總額
            var itemStr = string.Join("#", orderItems.Select(x => x.ProductName)); // 取得訂單項目名稱，使用 '#' 連接 (ecpay要求)

            // 建立結帳視圖模型
            CheckoutViewModel checkoutVM = new CheckoutViewModel()
            {
                // 建立綠界(ECPay)訂單內容
                EcpayOrder = GetEcpayOrder(orderId, itemStr, totalAmount),

                // 建立訂單資料
                Order = new Order
                {
                    OrderId = orderId,
                    UserId = user.Id,
                    UserName = user.UserName,
                    OrderItems = orderItems,
                    TotalAmount = totalAmount
                }
            };

            // 回傳結帳視圖模型
            return View(checkoutVM);
        }


        /// <summary>
        /// 處理建立訂單的請求
        /// </summary>
        /// <param name="order">訂單資料</param>
        /// <returns>建立訂單的結果</returns>
        [HttpPost]
        [Route("/api/Order/CreateOrder")]
        public async Task<IActionResult> CreateOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }

            using (var transcation = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (OrderExists(order.OrderId!))
                    {
                        return BadRequest("訂單編號已存在");
                    }

                    // 更新商品庫存數量
                    order.OrderItems = GetOrderItems(order.UserId!);
                    foreach (var item in order.OrderItems)
                    {
                        var target = await _context.Variant.FirstOrDefaultAsync(m => m.SKU == item.SKU);
                        if (target == null)
                        {
                            throw new ArgumentException("商品不存在");
                        }
                        else
                        {
                            await UpdateSKUStock(target, item.Quantity);
                        }
                    }

                    order.OrderDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    order.Status = 0;
                    order.PaymentType = "unknown";
                    order.CheckMacValue = "";
                    _context.Add(order);
                    await _context.SaveChangesAsync();
                    await transcation.CommitAsync();

                    await _connectionMultiplexer.GetDatabase(0).KeyDeleteAsync(order.UserId); // 移除購物車

                    return Ok();
                }
                catch (Exception ex)
                {
                    TempData["errorMsg"] = ex.Message;
                    return Json(new { redirectToUrl = Url.Action("Index", "Cart") });
                    //return RedirectToAction("Index", "Cart");
                }
            }

        }

        /// <summary>
        /// 更新庫存數量，處理庫存不足或資料庫更新衝突的情況。
        /// </summary>
        /// <param name="variant">庫存變體</param>
        /// <param name="quantity">更新的數量</param>
        /// <returns>Task</returns>
        public async Task UpdateSKUStock(Variant variant, int quantity)
        {
            bool retry;
            do
            {
                retry = false;
                try
                {
                    if (variant.Stock >= quantity)
                    {
                        variant.Stock -= quantity;
                        _context.Update(variant);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        throw new ArgumentException("庫存貨量不足");
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    retry = true;
                    var exEntry = ex.Entries.Single();
                    // var proposeValues = exEntry.CurrentValues;
                    var databaseValues = exEntry.GetDatabaseValues();
                    if (databaseValues == null)
                    {
                        throw new ArgumentException("商品資訊不存在");
                    }
                    else
                    {
                        // 更新庫存
                        var stk = databaseValues.GetValue<int>("Stock");
                        variant.Stock = stk;
                        // 更新RowVersion值
                        exEntry.OriginalValues.SetValues(databaseValues);
                    }
                }
            }
            while (retry);
        }


        /// <summary>
        /// 處理綠界(ECPay)付款回傳的請求
        /// </summary>
        /// <param name="myform">綠界(ECPay)付款回傳的表單資料</param>
        /// <returns>Http 回應訊息</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("/api/Order/Payment")]
        public HttpResponseMessage Payment(Dictionary<string,string> myform)
        {
            myform.TryGetValue("MerchantTradeNo", out string? orderId);
            if(orderId == null)
            {
                return ResponseError();
            }
            else
            {
                var order = _context.Order.FirstOrDefault(x => x.OrderId == orderId);
                if(order == null)
                {
                    return ResponseError();
                }
                else
                {
                    order.Status = 1;
                    _context.Update(order);
                    _context.SaveChanges();
                    return ResponseOK();
                }
            }
        }

        private HttpResponseMessage ResponseError()
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent("0|Error");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
        private HttpResponseMessage ResponseOK()
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent("1|OK");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }


        /// <summary>
        /// 自購物車取得訂購商品之內容
        /// </summary>
        /// <param name="userId">使用者Id</param>
        /// <returns></returns>
        private List<OrderItem> GetOrderItems(string userId)
        {
            var data = _connectionMultiplexer.GetDatabase(0).HashGetAll(userId).ToList();
            List<OrderItem> orderItems = new List<OrderItem>();
            foreach (var item in data)
            {
                var skuDetails = _context.Variant?
                    .Include(x => x.Product)
                    .FirstOrDefault(e => e.SKU == item.Name.ToString());
                OrderItem orderItem = new OrderItem();
                if (skuDetails != null)
                {
                    orderItem.SKU = skuDetails.SKU;
                    orderItem.ProductId = skuDetails.ProductId;
                    orderItem.ProductName = skuDetails.Product?.Name;
                    orderItem.Price = skuDetails.Product != null ? skuDetails.Product.Price : 0;
                    orderItem.Quantity = ((int)item.Value);
                    orderItem.SubTotal = orderItem.Quantity * orderItem.Price;
                    orderItems.Add(orderItem);
                }
            }
            return orderItems;
        }

        /// <summary>
        /// 取得購物訂單之總額
        /// </summary>
        /// <param name="orderItems"></param>
        /// <returns></returns>
        private int GetTotalAmount(List<OrderItem> orderItems)
        {
            var totalAmount = 0;
            foreach (var item in orderItems)
            {
                totalAmount += item.SubTotal;
            }
            return totalAmount;
        }

        /// <summary>
        /// 建立綠界(ECPay)訂單內容
        /// </summary>
        /// <param name="orderId">訂單編號</param>
        /// <param name="itemStr">訂單內容字串</param>
        /// <param name="totalAmount">訂單總額</param>
        /// <returns></returns>
        private Dictionary<string, string> GetEcpayOrder(string orderId, string itemStr, int totalAmount)
        {
            //var website = _config.GetValue<string>("Kestrel:Endpoints:Http:Url");
            var website = HttpContext.Request.Host;

            EcpayOrder ecpayOrder = new EcpayOrder()
            {
                MerchantID = "3002607",
                MerchantTradeNo = orderId,
                MerchantTradeDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                PaymentType = "aio",
                TotalAmount = totalAmount,
                ItemName = itemStr,
                TradeDesc = "null",
                ReturnURL = $"https://{website}/api/Order/Payment",
                OrderResultURL = $"https://{website}/Order",
                ChoosePayment = "Credit",
                EncryptType = 1,
            };

            // Convert Object to Dictionary
            var json = JsonConvert.SerializeObject(ecpayOrder);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            dictionary!.Remove("CheckMacValue");

            // CheckMacValue 為所需訂單內容之雜湊值
            dictionary["CheckMacValue"] = EcpayHelper.GetCheckMacValue(dictionary);

            return dictionary;
        }

        /// <summary>
        /// 確認訂單編號是否存在
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        private bool OrderExists(string orderId)
        {
            return _context.Order.Any(e => e.OrderId == orderId);
        }
        #endregion
    }
}
