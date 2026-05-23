import "dotenv/config";
import { createApp } from "./app.js";
import { getConfig } from "./config.js";

const config = getConfig();
const app = await createApp({ config });

try {
  await app.listen({ host: config.host, port: config.port });
} catch (error) {
  app.log.error(error);
  process.exit(1);
}
