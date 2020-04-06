FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic AS build-env

ENV PROJECT_NAME="DotnetCore.Api"

WORKDIR /app
COPY . ./

#RUN dotnet test
RUN dotnet restore src/$PROJECT_NAME/$PROJECT_NAME.csproj
RUN dotnet publish src/$PROJECT_NAME/$PROJECT_NAME.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-bionic
WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE 80
ENTRYPOINT ["dotnet", "DotnetCore.Api.dll"]

