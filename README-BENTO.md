# Bento Grid About Section

## Setup

```bash
dotnet restore
dotnet build
npm install
npm run build:css
dotnet run
```

## Features

- Responsive Bento Grid layout using Tailwind CSS 12-column grid
- Keyboard accessible tiles with Enter activation
- Micro-parallax on hover (disabled for touch devices and prefers-reduced-motion)
- Server-rendered with fallback to local fixtures
- WCAG AA compliant with visible focus states

## API Endpoint

Primary: `GET [PUBLIC_API_BASE_URL]/api/public/about/tiles`
Fallback: `data/fixtures/about-tiles.json`

## Testing

```bash
dotnet test
npx playwright test
```

## Security

- HTML content must be sanitized server-side (use HtmlSanitizer)
- CSS classes are validated against whitelist
- No secrets or API keys in code
