FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# COPY ["Ecommerce-app.csproj", "Ecommerce-app/"]
COPY ["Ecommerce-app/Ecommerce-app.csproj", "Ecommerce-app/"]
RUN dotnet restore "Ecommerce-app/Ecommerce-app.csproj"
COPY . .
WORKDIR "/src/Ecommerce-app"
RUN dotnet build "Ecommerce-app.csproj" -c Release -o /app/build

# 安装 dotnet-ef 工具
# RUN dotnet tool install --global dotnet-ef
# ENV PATH="$PATH:/root/.dotnet/tools"
# RUN dotnet ef database update --context UserContext
# RUN dotnet ef database update --context EcommerceAppContext

FROM build AS publish
RUN dotnet publish "Ecommerce-app.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .





ENTRYPOINT ["dotnet", "Ecommerce-app.dll"]
