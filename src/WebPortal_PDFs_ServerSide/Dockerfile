#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["../WebPortal_PDFs_ServerSide/WebPortal_PDFs_ServerSide.csproj", "../WebPortal_PDFs_ServerSide/"]
RUN dotnet restore "../WebPortal_PDFs_ServerSide/WebPortal_PDFs_ServerSide.csproj"
COPY . .
WORKDIR "/src/../WebPortal_PDFs_ServerSide"
RUN dotnet build "WebPortal_PDFs_ServerSide.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebPortal_PDFs_ServerSide.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebPortal_PDFs_ServerSide.dll"]