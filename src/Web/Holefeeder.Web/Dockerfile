###################################
# Restore stage
###################################
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS restore
WORKDIR /source

# Install Node.js, yarn and build-essential
RUN curl -fsSL https://deb.nodesource.com/setup_16.x | bash
RUN curl https://dl.yarnpkg.com/debian/pubkey.gpg | apt-key add -
RUN echo "deb https://dl.yarnpkg.com/debian/ stable main" | tee /etc/apt/sources.list.d/yarn.list
RUN apt-get update && apt-get install -y nodejs yarn build-essential

COPY *.sln .
COPY .editorconfig .
COPY global.json .
COPY Directory.Build.props .
COPY nuget.config .

COPY src/Api/Holefeeder.Api/*.csproj src/Api/Holefeeder.Api/
COPY src/Api/Holefeeder.Application/*.csproj src/Api/Holefeeder.Application/
COPY src/Api/Holefeeder.Domain/*.csproj src/Api/Holefeeder.Domain/
COPY src/Api/Holefeeder.Infrastructure/*.csproj src/Api/Holefeeder.Infrastructure/
COPY src/Web/Holefeeder.Web/*.csproj src/Web/Holefeeder.Web/

COPY src/Api/Holefeeder.UnitTests/*.csproj src/Api/Holefeeder.UnitTests/
COPY src/Api/Holefeeder.Tests.Common/*.csproj src/Api/Holefeeder.Tests.Common/
COPY src/Api/Holefeeder.FunctionalTests/*.csproj src/Api/Holefeeder.FunctionalTests/

RUN dotnet restore

###################################
# Build stage
###################################
FROM restore as build
ARG BUILD_VERSION=99.99.99
ARG VERSION=${BUILD_VERSION:-99.99.99}

WORKDIR /source
COPY src/. src/.
WORKDIR /source/src/Web/Holefeeder.Web

RUN dotnet publish /property:Version=${VERSION} -c Release -o /app --no-restore

###################################
# Debugger stage
###################################
FROM mcr.microsoft.com/dotnet/aspnet:7.0 as debugger
EXPOSE 80
EXPOSE 443
EXPOSE 22

WORKDIR /app
COPY --from=build /app ./

# Install openssh-server to enable SSH debugging
RUN apt-get update && \
    apt-get install -y  unzip procps curl bash openssh-server && \
    rm -rf /var/lib/apt/lists/*

# Configure SSH
RUN mkdir /run/sshd
RUN echo 'root:root' | chpasswd
RUN echo "PasswordAuthentication yes" >> /etc/ssh/sshd_config
RUN echo "PermitRootLogin yes" >> /etc/ssh/sshd_config
RUN update-rc.d ssh enable

# Start the SSH daemon and the application
ENTRYPOINT ["bash", "-c", "/etc/init.d/ssh start && dotnet Holefeeder.Web.dll"]

###################################
# Final stage
###################################
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
LABEL author="Drifter Apps Inc."
EXPOSE 80
EXPOSE 443

WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Holefeeder.Web.dll"]
