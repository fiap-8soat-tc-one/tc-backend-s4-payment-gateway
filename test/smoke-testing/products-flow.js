import { group } from "k6";

import { ShouldBeCreateProductReturnCreatedProduct } from "../scenarios/products/create-product.js";
import { ShouldBeUpdateProductReturnUpdatedProduct } from "../scenarios/products/update-product.js";
import { ShouldBeGetProductByIdReturnOneProduct } from "../scenarios/products/get-products.js";
import { ShouldBeGetProductByCategoryReturnsProduct } from "../scenarios/products/get-product-by-category.js";

export const options = {
  scenarios: {
    getApiV1Products: {
        executor: 'shared-iterations',
        vus: 1,
        iterations: 2,
        maxDuration: '5s',
        exec: "GetProducts"
    },
    getApiV1ProductById: {
      executor: 'shared-iterations',
      vus: 1,
      iterations: 2,
      maxDuration: '5s',
      exec: "GetProductByCategory"
    },
    postApiV1Product: {
      executor: 'shared-iterations',
      vus: 1,
      iterations: 2,
      maxDuration: '5s',
      exec: "CreateProduct"
    },
    putApiV1Product: {
      executor: 'shared-iterations',
      vus: 1,
      iterations: 2,
      maxDuration: '5s',
      exec: "UpdateProduct"
    },
  }
}

postman[Symbol.for("initial")]({
  options,
  collection: {
    BASE_URL: "http://localhost:8080"
  }
});

export function GetProducts() {
  group("Endpoint GET api/private/v1/products", () => { ShouldBeGetProductByIdReturnOneProduct() });
}

export function GetProductByCategory() {
  group("Endpoint GET api/api/public/v1/products/categories/{id}", () => { ShouldBeGetProductByCategoryReturnsProduct() });
}

export function CreateProduct() {
  group("Endpoint POST api/private/v1/products", () => { ShouldBeCreateProductReturnCreatedProduct() }); 
}

export function UpdateProduct() {
  group("Endpoint PUT api/private/v1/products", () => { ShouldBeUpdateProductReturnUpdatedProduct() }); 
}

