# Use the official .NET SDK image as the base image
FROM mcr.microsoft.com/devcontainers/dotnet:2-9.0-bookworm

# Install OpenSSH Server and supervisor
RUN apt-get update && apt-get install -y openssh-server supervisor

# Create the SSH directory and set permissions
RUN mkdir -p /var/run/sshd
RUN chmod 0755 /var/run/sshd

# Create a config directory in your project and copy the supervisord.conf file
# Make sure you create a 'config' directory at the root of your project,
# and inside it, create a 'supervisord.conf' file
COPY config/supervisord.conf /etc/supervisor/conf.d/supervisord.conf

# Disable password authentication
RUN sed -i 's/#PasswordAuthentication yes/PasswordAuthentication no/' /etc/ssh/sshd_config
RUN sed -i 's/PasswordAuthentication yes/PasswordAuthentication no/' /etc/ssh/sshd_config

# Expose the SSH port
EXPOSE 22

# Start supervisord, which in turn will start the SSH service
CMD ["/usr/bin/supervisord"]
