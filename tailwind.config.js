/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './Views/**/*.cshtml',
    './wwwroot/js/**/*.js',
    './Pages/**/*.cshtml',
    './Areas/**/Views/**/*.cshtml'
  ],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        // Three-color scheme: black, dark blue, white
        'void-black': '#000000',
        'charcoal': '#000000',
        'accent': '#001f3f',
        'accent-glow': '#003366',
        'canvas': 'var(--canvas)',
        'ink': 'var(--ink)',
        // Legacy support
        'accent-dark': '#001f3f',
        'accent-alt': '#002b5c',
        'primary': '#001f3f',
        'secondary': '#001f3f',
      },
      fontFamily: {
        'clash': ['Clash Display', 'Inter', 'system-ui', 'sans-serif'],
        'satoshi': ['Satoshi', '-apple-system', 'BlinkMacSystemFont', 'Segoe UI', 'sans-serif'],
        heading: ['Clash Display', 'Inter', 'sans-serif'],
        body: ['Satoshi', 'Inter', 'sans-serif']
      },
      animation: {
        'reveal': 'reveal 0.8s cubic-bezier(0.16, 1, 0.3, 1) forwards',
        'float': 'float 6s ease-in-out infinite',
        'glow-pulse': 'glow-pulse 2s ease-in-out infinite alternate',
      },
      keyframes: {
        reveal: {
          '0%': { opacity: '0', transform: 'translateY(20px)' },
          '100%': { opacity: '1', transform: 'translateY(0)' },
        },
        float: {
          '0%, 100%': { transform: 'translateY(0px)' },
          '50%': { transform: 'translateY(-10px)' },
        },
        'glow-pulse': {
          '0%': { boxShadow: '0 0 20px rgba(79, 70, 229, 0.3)' },
          '100%': { boxShadow: '0 0 40px rgba(79, 70, 229, 0.6)' },
        },
      },
      backdropBlur: {
        'xs': '2px',
      },
    },
  },
  plugins: [],
}
