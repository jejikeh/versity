import { Product } from "../../Product";
import { SessionStatus } from "../SessionStatus";

export interface GetSessionByIdViewModel {
    id: string;
    userId: string;
    product: Product;
    logsId: string;
    start: Date;
    expiry: Date;
    status: SessionStatus;
}