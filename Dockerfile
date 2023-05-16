FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
COPY . ./
WORKDIR /PushListenerForLinux/
RUN dotnet dev-certs https
EXPOSE 8080
EXPOSE 7181
EXPOSE 5181
Expose 4060 
Expose 4062
Expose 4063
ENTRYPOINT ["dotnet","run"]