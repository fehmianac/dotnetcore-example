# dotnet 3.1 Example

dotnet 3.1 ile geliştirilmiş ve dockerize edilmiş örnek uygulamadır. Uygulamanın ayağa kalkabilemesi için **ASPNETCORE_ENVIRONMENT** environment parametresi ile çalıştırılmalıdır.

Uygulamanın 3 adet rest api route'u vardır.

  - **/api/v{version}/Example/users** -> in memory den user listesi getirmek için kullanılır.
  
  - **/api/v{version}/Example/user-id-from-db** -> MSSQL veritabanından NEWID() dönecek şekişde programlanmıştır. Bu örneğim çalışması için appsettings.*.json içerisine connection string yazılmalıdır.
  
  - **/api/v{version}/Example/users-with-cache** -> in memory den user listesi getirip redis'e yazdığını gözlemlemek için kullanılır. Bu örneğim çalışması için appsettings.*.json içerisine redis bilgileri yazılmalıdır.

Uygulamanın sağlıklı çalışığ çalışmadığını öğrenmek için **/health** route'unun http_status_code **200** dönmesi gerekmektedir.

## Unit Test Komutu

```bash
dotnet test
```
## Docker Komutları

```bash
docker build -t dotnetcore-example .

docker run -e ASPNETCORE_ENVIRONMENT=Production -p 8081:80 dotnetcore-example:latest
```

Yukarıdaki komutlar çalıştırıldıktan sonra bu [link](http://localhost:8081/swagger/index.html) ile ulaşabilirsiniz.

