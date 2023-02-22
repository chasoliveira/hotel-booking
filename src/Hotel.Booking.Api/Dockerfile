#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["src/Hotel.Common/Hotel.Common.csproj", "Hotel.Common/"]
COPY ["src/Hotel.Booking.Api/Hotel.Booking.Api.csproj", "Hotel.Booking.Api/"]

RUN dotnet restore "Hotel.Booking.Api/Hotel.Booking.Api.csproj"
COPY . .

WORKDIR "./src/Hotel.Booking.Api"
RUN dotnet build "Hotel.Booking.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Hotel.Booking.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hotel.Booking.Api.dll"]