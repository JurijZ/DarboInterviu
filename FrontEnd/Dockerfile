FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["FrontEnd/FrontEnd.csproj", "FrontEnd/"]
RUN dotnet restore "FrontEnd/FrontEnd.csproj"
COPY . .
WORKDIR "/src/FrontEnd"
RUN dotnet build "FrontEnd.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "FrontEnd.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FrontEnd.dll"]