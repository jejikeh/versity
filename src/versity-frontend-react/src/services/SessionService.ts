import $api from "../https"
import { Axios, AxiosResponse } from "axios"
import { User } from "../models/User"
import { UserSessionsViewModel } from "../models/session/queries/UserSessionsViewModel"
import { CreateSessionDto } from "../models/session/commands/CreateSessionDto"
import { GetSessionByIdViewModel } from "../models/session/queries/GetSessionByIdViewModel"
import { SessionViewModel } from "../models/session/queries/SessionViewModel"

export default class SessionService {
  static async getSessions(page: number): Promise<AxiosResponse<SessionViewModel[]>> {
    return $api.get<SessionViewModel[]>('/sessions/' + page)
  }

  static async getSessionById(id: string): Promise<AxiosResponse<GetSessionByIdViewModel>> {
    return $api.get<GetSessionByIdViewModel>('/sessions/' + id)
  }

  static async getUserSessions(userId: string, page: number): Promise<AxiosResponse<UserSessionsViewModel[]>> {
    return $api.get<UserSessionsViewModel[]>('/sessions/users/' + userId + '/' + page)
  }

  static async createSession(dto: CreateSessionDto): Promise<AxiosResponse<SessionViewModel>> {
    return $api.post('/sessions', dto)
  }
}
