import react from "@vitejs/plugin-react";
import { defineConfig } from "vite";

export default defineConfig({
  base: "./",
  plugins: [react()],
  build: {
    outDir: "../website/forum",
    emptyOutDir: true,
    rollupOptions: {
      output: {
        entryFileNames: "assets/forum.js",
        chunkFileNames: "assets/[name].js",
        assetFileNames: (assetInfo) => {
          return assetInfo.name?.endsWith(".css") ? "assets/forum.css" : "assets/[name][extname]";
        }
      }
    }
  },
  server: {
    port: 5173
  },
  preview: {
    port: 4178
  }
});
