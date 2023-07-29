export interface CreateSessionDto {
    userId: string;
    productId: string;
    start: Date;
    expiry: Date;
}