import { group } from "k6";

import { ShouldBeCreateOrderReturnCreatedOrder } from "../scenarios/orders/create-order.js";
import { ShouldBeUpdateOrderStatusReturnUpdatedOrder } from "../scenarios/orders/update-order-status.js";
import { ShouldBeGetOrdersReturnOneOrMoreOrder } from "../scenarios/orders/get-orders.js";
import { ShouldBeGetOrderByIdReturnsOrder } from "../scenarios/orders/get-order-by-id.js";

export const options = {
  scenarios: {
    getApiV1Orders: {
        executor: 'shared-iterations',
        vus: 1,
        iterations: 2,
        maxDuration: '5s',
        exec: "GetOrders"
    },
    getApiV1OrderById: {
      executor: 'shared-iterations',
      vus: 1,
      iterations: 2,
      maxDuration: '5s',
      exec: "GetOrderById"
    },
    postApiV1Order: {
      executor: 'shared-iterations',
      vus: 1,
      iterations: 2,
      maxDuration: '5s',
      exec: "CreateOrder"
    },
    putApiV1Order: {
      executor: 'shared-iterations',
      vus: 1,
      iterations: 2,
      maxDuration: '5s',
      exec: "UpdateOrderStatus"
    },
  }
}

postman[Symbol.for("initial")]({
  options,
  collection: {
    BASE_URL: "http://localhost:8080"
  }
});

export function GetOrders() {
  group("Endpoint GET api/public/v1/orders", () => { ShouldBeGetOrdersReturnOneOrMoreOrder() });
}

export function GetOrderById() {
  group("Endpoint GET api/private/v1/orders/{id}", () => { ShouldBeGetOrderByIdReturnsOrder() });
}

export function CreateOrder() {
  group("Endpoint PUT api/private/v1/orders", () => { ShouldBeCreateOrderReturnCreatedOrder() }); 
}

export function UpdateOrderStatus() {
  group("Endpoint PUT api/private/v1/orders/status", () => { ShouldBeUpdateOrderStatusReturnUpdatedOrder() }); 
}