# Unity WebGL build + GitHub Pages deploy

Este repositório contém um projeto Unity configurado para gerar builds WebGL e publicá-los automaticamente em uma URL pública.

## Visão geral

- **Cena principal**: `Assets/Scenes/Loading.unity` é uma tela de loading responsiva que pode carregar uma próxima cena ou apresentar uma animação simulada de progresso.
- **Template WebGL**: `Assets/WebGLTemplates/Minimal/index.html` é um template leve com barra de progresso, responsivo para mobile e desktop, que utiliza *Brotli* por padrão.
- **Automação de build**: GitHub Actions (`.github/workflows/webgl-build-deploy.yml`) usa o [game-ci/unity-builder](https://game.ci/docs/github/getting-started) para compilar WebGL sempre que houver push na branch `main` e faz deploy contínuo para GitHub Pages.
- **Player Settings**: definidos por script em `Assets/Editor/Build/WebGLBuild.cs` para garantir WebGL 2.0, compressão Brotli, cache de dados, `memorySize` de 256 MB, janela redimensionável e escala responsiva.

## Pipeline CI/CD

1. **Build**
   - Executado em `ubuntu-latest`.
   - Requer o segredo `UNITY_LICENSE` contendo a licença Unity (personal ou professional) exportada via `unity -batchmode -createManualActivationFile`.
   - Usa cache da pasta `Library` baseado no `manifest.json`.
   - Executa `WebGLBuild.Build` que aplica os PlayerSettings e invoca o `BuildPipeline` para o target `WebGL`.

2. **Deploy**
   - Faz download do artefato WebGL gerado no passo anterior.
  - Publica o conteúdo via GitHub Pages usando `actions/deploy-pages`.
  - A URL final fica disponível no ambiente `github-pages` (visível no summary da execução).

## Como habilitar o deploy público

1. Acesse **Settings → Secrets and variables → Actions** do repositório e cadastre:
   - `UNITY_LICENSE`: bloco de texto da licença Unity em formato `.ulf`.

2. Vá em **Settings → Pages** e escolha "GitHub Actions" como fonte.

3. Após o primeiro push na `main`, o workflow `Build and deploy WebGL` irá gerar a build e publicar automaticamente. A URL pública seguirá o padrão:

   ```text
   https://<usuario-ou-organizacao>.github.io/<nome-do-repositorio>/
   ```

   O link exato aparece como output do job `Deploy to GitHub Pages` (campo `page_url`).

## Desenvolvimento local

1. Instale o Unity `2021.3.33f1` com o módulo **WebGL**.
2. Abra o projeto (`File → Open Project`) apontando para a raiz deste repositório.
3. Utilize a cena `Loading.unity` como ponto de partida.
4. Para gerar builds locais execute `Build/Build WebGL (CI)` no menu do Unity Editor.

## Estrutura principal

```
Assets/
├── Editor/Build/WebGLBuild.cs      # Script de build e configuração dos PlayerSettings
├── Scenes/Loading.unity            # Cena de loading responsiva
├── Scripts/LoadingController.cs    # Cria UI de loading em tempo de execução
└── WebGLTemplates/Minimal/index.html
ProjectSettings/
├── ProjectSettings.asset
├── ProjectVersion.txt
└── EditorBuildSettings.asset
```

## Próximos passos

- Criar cenas adicionais do jogo e adicioná-las ao **Build Settings** para que a tela de loading carregue o conteúdo real.
- Personalizar o template `Assets/WebGLTemplates/Minimal` com branding próprio, ícones e assets adicionais.
- Ajustar `fallbackDuration` e `sceneToLoad` no `LoadingController` conforme a lógica de carregamento desejada.

Com isso, qualquer alteração publicada na branch `main` resultará em uma nova versão jogável diretamente no navegador através do GitHub Pages.
