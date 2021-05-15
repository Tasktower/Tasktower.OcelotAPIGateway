FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
COPY . Tasktower.OcelotAPIGateway
WORKDIR  /Tasktower.OcelotAPIGateway/Tasktower.OcelotGateway
RUN dotnet restore "Tasktower.OcelotGateway.csproj"
RUN dotnet publish "Tasktower.OcelotGateway.csproj" -c release -o /App --no-restore
FROM mcr.microsoft.com/dotnet/aspnet:5.0
EXPOSE 5001
EXPOSE 5000
EXPOSE 443
EXPOSE 80
WORKDIR /App
COPY --from=build /App ./
ENTRYPOINT ["dotnet", "Tasktower.OcelotGateway.dll"]