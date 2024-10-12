FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SupaGPT.csproj", "./"]
RUN dotnet restore "SupaGPT.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "SupaGPT.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SupaGPT.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080

CMD ["dotnet", "SupaGPT.dll", "--urls=http://0.0.0.0:8080"]
