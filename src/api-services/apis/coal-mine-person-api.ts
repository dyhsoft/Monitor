/* tslint:disable */
/* eslint-disable */
import globalAxios, { AxiosRequestConfig } from 'axios';
import { Configuration } from '../configuration';
import { BASE_PATH } from '../base';

// 人员信息
export class PersonInfoApi {
    private configuration: Configuration;
    private basePath: string;

    constructor(configuration: Configuration, basePath: string = BASE_PATH) {
        this.configuration = configuration;
        this.basePath = basePath;
    }

    async getPage(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/personInfo/page`, { params, ...this.configuration, ...options });
    }

    async getList(mineId: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/personInfo/list`, { params: { mineId }, ...this.configuration, ...options });
    }

    async get(id: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/personInfo/${id}`, { ...this.configuration, ...options });
    }

    async add(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.post(`${this.basePath}/api/personInfo`, data, { ...this.configuration, ...options });
    }

    async update(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.put(`${this.basePath}/api/personInfo`, data, { ...this.configuration, ...options });
    }

    async delete(id: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.delete(`${this.basePath}/api/personInfo/${id}`, { ...this.configuration, ...options });
    }
}

// 人员出勤
export class PersonAttendanceApi {
    private configuration: Configuration;
    private basePath: string;

    constructor(configuration: Configuration, basePath: string = BASE_PATH) {
        this.configuration = configuration;
        this.basePath = basePath;
    }

    async getPage(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/personAttendance/page`, { params, ...this.configuration, ...options });
    }

    async getList(mineId: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/personAttendance/list`, { params: { mineId }, ...this.configuration, ...options });
    }

    async get(id: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/personAttendance/${id}`, { ...this.configuration, ...options });
    }

    async add(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.post(`${this.basePath}/api/personAttendance`, data, { ...this.configuration, ...options });
    }

    async update(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.put(`${this.basePath}/api/personAttendance`, data, { ...this.configuration, ...options });
    }

    async delete(id: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.delete(`${this.basePath}/api/personAttendance/${id}`, { ...this.configuration, ...options });
    }
}
