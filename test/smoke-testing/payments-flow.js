import { group } from "k6";
import { ShouldBeSuccessReceiveWebHookOrderPayment } from "../scenarios/payments/webhook.js";

export const options = {
  scenarios: {
    webHook: {
        executor: 'shared-iterations',
        vus: 1,
        iterations: 2,
        maxDuration: '5s',
        exec: "ReceiveWebHookOrderPayment"
    },
  }
}

postman[Symbol.for("initial")]({
    options,
    collection: {
      BASE_URL: "http://localhost:8080"
    }
  });
  
export function ReceiveWebHookOrderPayment() {
    group("Endpoint POST api/public/v1/hook/orders/payments", () => { ShouldBeSuccessReceiveWebHookOrderPayment() });
  }