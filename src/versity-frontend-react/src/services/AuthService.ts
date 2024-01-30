import $api from "../https"
import { Axios, AxiosResponse } from "axios"
import { LoginResponse } from "../models/LoginResponse"

export default class AuthService {
    static async login(email: string, password: string) : Promise<AxiosResponse<LoginResponse>> {
        return $api.post<LoginResponse>('/auth/login', {email, password})
    }

    static async register(firstName: string, lastName: string, email: string, phone: string, password: string) : Promise<AxiosResponse<string>> {
        return $api.post<string>('/auth/register', {firstName, lastName, email, phone, password})
    }
}