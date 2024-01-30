import $api from "../https"
import { Axios, AxiosResponse } from "axios"
import {User} from "../models/User"

export default class UserService {
    static async fetchUsers(page: number) : Promise<AxiosResponse<User[]>> {
        return $api.get<User[]>('/users/' + page)
    }
}