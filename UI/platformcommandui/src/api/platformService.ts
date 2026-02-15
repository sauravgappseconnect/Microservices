import axios from 'axios';
import { type PlatformResponseSchema } from "../types/commandService";

const platformServiceUrl = import.meta.env.VITE_PLATFORM_SERVICE_URL;

const requestInstance = axios.create({
    baseURL: `${platformServiceUrl}/api`,
    timeout: 5000,
});

const delayer = function () {
    return new Promise((resolve) => {
        setTimeout(() => {
            resolve("delayed");
        }, 3000);
    });
}

requestInstance.interceptors.request.use(async onfullfilled => {
    await delayer();
    return onfullfilled;
});

const getPlatformServicePlatforms = async function () {
    const result = await requestInstance.get<PlatformResponseSchema[]>('/platform/GetAllPlatforms');
    return result.data;
}

const editPlatformService = async function (id: string) {
    const result = await requestInstance.get<PlatformResponseSchema>(`/platform/GetById?id=${id}`);
    return result.data;
}

const updatePlatformService = async function (value: PlatformResponseSchema) {
    await requestInstance.put(`platform?id=${value.id}`, value);
}

const createPlatformService = async function (value: PlatformResponseSchema) {
    await requestInstance.post(`platform`, value);
}

export { getPlatformServicePlatforms, editPlatformService, updatePlatformService, createPlatformService };