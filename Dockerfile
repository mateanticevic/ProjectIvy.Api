ARG version=1.0.0
FROM mcr.microsoft.com/dotnet/aspnet:10.0-bookworm-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .
WORKDIR "/src/src/ProjectIvy.Api"
RUN dotnet build "ProjectIvy.Api.csproj" -c Release -o /app

FROM build AS publish
ARG version
RUN echo $version
RUN dotnet publish "ProjectIvy.Api.csproj" -c Release -o /app /p:Version=$version

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ProjectIvy.Api.dll"]
