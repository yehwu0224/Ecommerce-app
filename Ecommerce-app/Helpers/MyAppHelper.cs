namespace Ecommerce_app.Helpers
{
    public class MyAppHelper
    {
        /// <summary>
        /// 轉換圖片為位元組序列
        /// </summary>
        /// <param name="image">表單中的圖檔</param>
        /// <returns></returns>
        public static byte[] ImageToByteArray(IFormFile image)
        {
            using (var ms = new MemoryStream())
            {
                image.CopyTo(ms);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 轉換位元組序列為Base64編碼圖檔
        /// </summary>
        /// <param name="arrayImage">資料庫取出的位元組圖檔</param>
        /// <returns></returns>
        public static string ViewImage(byte[] arrayImage)
        {
            if (arrayImage == null)
            {
                return "";
            }
            else
            {
                string base64String = Convert.ToBase64String(arrayImage, 0, arrayImage.Length);
                return "data:image/png;base64," + base64String;
            }
        }
    }
}
