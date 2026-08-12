/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        slate: {
          950: '#090714',
          900: '#130f26',
          800: '#21193d',
          700: '#34285e',
          600: '#4c3b88',
        },
        brand: {
          50: '#f5f3ff',
          100: '#ede9fe',
          200: '#ddd6fe',
          300: '#c4b5fd',
          400: '#a78bfa',
          500: '#8b5cf6',
          600: '#7c3aed',
          700: '#6d28d9',
          800: '#5b21b6',
          900: '#4c1d95',
          950: '#2e1065',
        },
        sla: {
          ok: {
            bg: '#f0fdf4',
            text: '#15803d',
            border: '#bbf7d0',
          },
          warning: {
            bg: '#fffbe6',
            text: '#b45309',
            border: '#fef08a',
          },
          overdue: {
            bg: '#fef2f2',
            text: '#dc2626',
            border: '#fecaca',
          }
        }
      }
    },
  },
  plugins: [],
}
