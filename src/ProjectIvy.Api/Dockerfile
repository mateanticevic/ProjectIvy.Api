FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1803 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk-nanoserver-1803 AS build
WORKDIR /src
COPY ["src/ProjectIvy.Api/ProjectIvy.Api.csproj", "src/ProjectIvy.Api/"]
COPY ["src/ProjectIvy.Common/ProjectIvy.Common.csproj", "src/ProjectIvy.Common/"]
COPY ["src/ProjectIvy.BL/ProjectIvy.BL.csproj", "src/ProjectIvy.BL/"]
COPY ["src/ProjectIvy.DL/ProjectIvy.DL.csproj", "src/ProjectIvy.DL/"]
COPY ["src/ProjectIvy.Model/ProjectIvy.Model.csproj", "src/ProjectIvy.Model/"]
RUN dotnet restore "src/ProjectIvy.Api/ProjectIvy.Api.csproj"
COPY . .
WORKDIR "/src/src/ProjectIvy.Api"
RUN dotnet build "ProjectIvy.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ProjectIvy.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ProjectIvy.Api.dll"]