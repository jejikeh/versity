import AuthService from "../services/AuthService";
import { User } from "../models/User";
import { makeAutoObservable } from "mobx";
import axios from "axios";
import { LoginResponse } from "../models/LoginResponse";
import { API_URL } from "../https";

export default class Store {
    refreshtoken = "";
    userId = "";
    isAuth = false;
    
    constructor() {
        makeAutoObservable(this)
    }

    setAuth(bool: boolean) {
        this.isAuth = bool;
    }

    setRefteshToken(token: string) {
        this.refreshtoken = token;
    }

    setUserId(id: string) {
        this.userId = id;
    }

    async login(email: string, password: string) {
        try {
            const response = await AuthService.login(email, password);
            localStorage.setItem('token', response.data.token);
            localStorage.setItem('refresh-token', response.data.refreshToken);
            localStorage.setItem('id', response.data.id);
            this.setAuth(true);
            this.setRefteshToken(response.data.refreshToken);
            this.setUserId(response.data.id);
        } catch (e) {

        }
    }

    async registraction(firstName: string, lastName: string, email: string, phone: string, password: string) {
        try {
            const response = await AuthService.register(firstName, lastName, email, phone, password);
        } catch (e) {

        }
    }

    async logout() {
        try {
            localStorage.removeItem('token');
            localStorage.removeItem('refresh-token');
            localStorage.removeItem('id');
            this.setAuth(false);
            this.setRefteshToken("");
            this.setUserId("");
        } catch (e) {

        }
    }

    async checkAuth() {
        try {
            const response = await axios.post<LoginResponse>(`${API_URL}/auth/refreshtoken/${localStorage.getItem("id")}/${localStorage.getItem("refresh-token")}`);
            localStorage.setItem('token', response.data.token);
            this.setAuth(true);
            this.setRefteshToken(response.data.refreshToken);
            this.setUserId(response.data.id);
        } catch (e) {

        }
    }
}