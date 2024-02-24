# Dependencies.
FROM continuumio/miniconda3 as miniconda-base
RUN conda update -n base -c defaults conda
FROM selenium/standalone-chrome as chrome-base

# Start with the .NET SDK image.
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
# Set the working directory.
WORKDIR /app
# Copy the source code.
COPY ./src .
# Restore dependencies.
RUN dotnet restore
# Build the application.
RUN dotnet build -c Release --no-restore
# Publish the application.
RUN dotnet publish -c Release --no-build -o /app/publish
# Start with a smaller runtime image.
FROM mcr.microsoft.com/dotnet/runtime:7.0 AS runtime
# Copy the entire Miniconda installation from the first image
COPY --from=miniconda-base /opt/conda /opt/conda
COPY --from=miniconda-base /opt/conda /root/miniconda3
# Add conda to the PATH and initialize in the bash config file (.bashrc)
ENV PATH /opt/conda/bin:$PATH
RUN echo ". /opt/conda/etc/profile.d/conda.sh" >> ~/.bashrc && echo "conda activate base" >> ~/.bashrc
# Initialize conda for shell interaction
SHELL ["/bin/bash", "--login", "-c"]
# Update apt repositories and install ffmpeg
RUN apt-get update && apt-get install -y ffmpeg libnss3 && apt-get clean && rm -rf /var/lib/apt/lists/*
# Install chrome webdriver.
COPY --from=chrome-base /opt/selenium /opt/selenium
COPY --from=chrome-base /opt/bin /opt/bin
ENV PATH="/opt/bin:${PATH}"
ENV PATH="/opt/selenium:${PATH}"
# Install git.
RUN apt-get update && apt-get install git -y
ENV PATH="/usr/local/bin:${PATH}"
RUN git config --global http.sslverify false
# Set the working directory.
WORKDIR /app
# Copy the published output from the build stage.
COPY --from=build /app/publish .
# Set the entry point for the application
ENTRYPOINT ["dotnet", "FrostAura.Intelligence.Iluvatar.Telegram.dll"]