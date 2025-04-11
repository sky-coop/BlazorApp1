# 使用 .NET 8 SDK 构建镜像
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 复制 csproj 并还原依赖项
COPY ["BlazorApp1/BlazorApp1.csproj", "BlazorApp1/"]
RUN dotnet restore "BlazorApp1/BlazorApp1.csproj"

# 复制其余源码
COPY . .
WORKDIR "/src/BlazorApp1"
RUN dotnet publish "BlazorApp1.csproj" -c Release -o /app/publish

# 构建运行时镜像
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BlazorApp1.dll"]
