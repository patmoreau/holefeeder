###################################
# Node and yarn install stage
###################################
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS pre-build
WORKDIR /source

# Download and import the Nodesource GPG key
RUN apt-get update
RUN apt-get install -y ca-certificates curl gnupg
RUN mkdir -p /etc/apt/keyrings
RUN curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg

# Create deb repository
ENV NODE_MAJOR=20
RUN echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_$NODE_MAJOR.x nodistro main" | tee /etc/apt/sources.list.d/nodesource.list

# Run Update and Install
RUN apt-get update
RUN apt-get install nodejs -y

# Install yarn and build-essential
RUN curl -fsSL https://dl.yarnpkg.com/debian/pubkey.gpg | gpg --dearmor -o /etc/apt/keyrings/yarnpkg.gpg
RUN echo "deb [signed-by=/etc/apt/keyrings/yarnpkg.gpg] https://dl.yarnpkg.com/debian/ stable main" | tee /etc/apt/sources.list.d/yarn.list
RUN apt-get update
RUN apt-get install -y yarn build-essential

###################################
# Restore stage
###################################
FROM pre-build AS restore

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
FROM restore AS build
ARG BUILD_VERSION=99.99.99
ARG VERSION=${BUILD_VERSION:-99.99.99}

WORKDIR /source
COPY src/. src/.
WORKDIR /source/src/Web/Holefeeder.Web

RUN dotnet publish /property:Version=${VERSION} -c Release -o /app --no-restore

###################################
# Unit tests stage
###################################
FROM build AS angulartests
WORKDIR /source/src/Web/Holefeeder.Web/ClientApp

RUN wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add -
RUN sh -c 'echo "deb http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list'
RUN apt-get update
RUN apt --fix-broken install
RUN apt-get install google-chrome-stable -y

###################################
# Debugger stage
###################################
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS debugger
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
