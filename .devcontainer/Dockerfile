# netcoreapp2.1 builds on modern SDK; the official sdk:2.1 image uses EOL Debian Stretch (apt 404).
FROM mcr.microsoft.com/dotnet/sdk:8.0

ARG DEBIAN_FRONTEND=noninteractive

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        bash \
        ca-certificates \
        git \
        jq \
        procps \
        python3 \
        unzip \
    && rm -rf /var/lib/apt/lists/*

ARG USERNAME=vscode
ARG USER_UID=1000
ARG USER_GID=$USER_UID
RUN groupadd --gid $USER_GID $USERNAME \
    && useradd --uid $USER_UID --gid $USER_GID -m $USERNAME \
    && mkdir -p /workspace \
    && chown -R $USER_GID:$USER_GID /workspace

USER $USERNAME
WORKDIR /workspace
