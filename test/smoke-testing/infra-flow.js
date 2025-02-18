import { group } from "k6";
import { ShouldBeGetVerifyApplicationIsHealthy } from "../scenarios/infra/health-check.js";

export const options = {
  scenarios: {
    healthCheck: {
        executor: 'shared-iterations',
        vus: 1,
        iterations: 2,
        maxDuration: '5s',
        exec: "GetVerifyApplicationIsHealthy"
    },
  }
}

postman[Symbol.for("initial")]({
    options,
    collection: {
      BASE_URL: "http://localhost:8080"
    }
  });
  
export function GetVerifyApplicationIsHealthy() {
    group("Endpoint GET api/public/v1/health", () => { ShouldBeGetVerifyApplicationIsHealthy() });
  }