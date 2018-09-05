
FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
RUN mkdir Plugins
FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY NotifyMe/NotifyMe.csproj NotifyMe/NotifyMe/
COPY NotifyMe.Common/NotifyMe.Common.csproj NotifyMe/NotifyMe/
COPY NotifyMe.Templates/NotifyMe.Templates.csproj NotifyMe/NotifyMe/

WORKDIR /src/NotifyMe/NotifyMe
COPY . .
RUN dotnet build NotifyMe.Templates/NotifyMe.Templates.csproj -c Release -o /app/Plugins
RUN dotnet build NotifyMe/NotifyMe.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish NotifyMe/NotifyMe.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "NotifyMe.dll"]
