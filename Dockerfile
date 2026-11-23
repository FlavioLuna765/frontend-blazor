FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY Fcg.Web.csproj ./

RUN dotnet restore Fcg.Web.csproj

COPY . .

RUN dotnet publish Fcg.Web.csproj -c Release -o /app/publish --force

FROM nginx:alpine AS final

COPY nginx.conf /etc/nginx/conf.d/default.conf

COPY --from=build /app/publish/wwwroot /usr/share/nginx/html

EXPOSE 80

ENTRYPOINT ["nginx", "-g", "daemon off;"]
