# WebGL Build and Deployment

This repository is configured to build and publish a Unity WebGL project using GitHub Actions and GitHub Pages.

## Continuous Deployment

- The `Build WebGL and Deploy Pages` workflow builds the project with [`game-ci/unity-builder@v4`](https://github.com/game-ci/unity-builder) targeting Unity **2022.3.37f1** and uploads the generated files from `build/WebGL` as a GitHub Pages artifact.
- Deployment to Pages is handled by [`actions/deploy-pages@v4`](https://github.com/actions/deploy-pages), making the WebGL build available after every push to the `main` branch or when triggered manually.

### Required Secrets

To run the workflow you must provide the Unity credentials that fit your license setup:

- `UNITY_LICENSE` for personal or floating licenses (base64 encoded).
- Optional: `UNITY_EMAIL`, `UNITY_PASSWORD`, `UNITY_SERIAL` for Professional or Plus licenses.

### Repository Settings

1. **Settings → Actions → General → Workflow permissions** must be set to **Read and write**.
2. **Settings → Pages → Source** must be **GitHub Actions** so deployments land on the GitHub Pages environment.

## Live Build

Once the workflow finishes successfully, the latest WebGL build is published at:

[https://<owner>.github.io/<repo>/](https://<owner>.github.io/<repo>/)

Update the link above with your GitHub organisation or username and repository name if needed.
