#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Idams.Core/Idams.Core.csproj", "Idams.Core/Idams.Core.csproj"]
COPY ["Idams.Infrastructure/Idams.Infrastructure.csproj", "Idams.Infrastructure/Idams.Infrastructure.csproj"]
COPY ["Idams.Infrastructure.EntityFramework/Idams.Infrastructure.EntityFramework.csproj", "Idams.Infrastructure.EntityFramework/Idams.Infrastructure.EntityFramework.csproj"]
COPY ["Idams.WebApi/Idams.WebApi.csproj", "Idams.WebApi/Idams.WebApi.csproj"]
COPY ["Idams.WebApi.Utils/Idams.WebApi.Utils.csproj", "Idams.WebApi.Utils/Idams.WebApi.Utils.csproj"]
COPY ["Idams.WebApi.sln", "."]
RUN dotnet restore -v diag
COPY . .
WORKDIR "/src/."
RUN dotnet build "Idams.WebApi/Idams.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Idams.WebApi/Idams.WebApi.csproj" -c Release -o /app/publish
#
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Idams.WebApi.dll"]