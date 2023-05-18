# Use the official Anaconda base image with Python 3.8
FROM continuumio/miniconda3:latest

# Set the working directory inside the container
WORKDIR /app

# Copy the source code into the container
COPY ./src .

# Create a new conda environment
RUN conda create --name zeus python=3.8

# Activate the conda environment
RUN echo "source activate zeus" >> ~/.bashrc
SHELL ["/bin/bash", "--login", "-c"]

# Install pip dependencies inside the conda environment
RUN conda run --name zeus pip install -r requirements.txt

# Set the entrypoint to run the zeus_manager.py app in the conda environment
ENTRYPOINT ["conda", "run", "--name", "zeus", "python", "/app/app.py"]