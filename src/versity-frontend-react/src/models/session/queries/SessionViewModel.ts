import { Product } from "../../Product";
import { SessionStatus } from "../SessionStatus";

export interface SessionViewModel {
    id: string;
    userId: string;
    productId: string;
    logsId: string;
    start: Date;
    expiry: Date;
    status: SessionStatus;
}