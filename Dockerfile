# 使用 .NET 8 ASP.NET Core 基础镜像
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# 使用 .NET SDK 构建项目
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 拷贝 csproj 并还原依赖项
COPY ["BlazorApp1.csproj", "./"]
RUN dotnet restore "./BlazorApp1.csproj"

# 拷贝项目代码并构建
COPY . .
RUN dotnet publish "BlazorApp1.csproj" -c Release -o /app/publish

# 发布镜像
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BlazorApp1.dll"]
