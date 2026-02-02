import axios from 'axios';
import  { type PlatformResponseSchema } from "../types/commandService";

const platformServiceUrl = import.meta.env.VITE_PLATFORM_SERVICE_URL;

const requestInstance = axios.create({
    baseURL: `${platformServiceUrl}/api`,
    timeout: 5000,
});

const getPlatformServicePlatforms = async function () {
    const result = await requestInstance.get<PlatformResponseSchema[]>('/platform/GetAllPlatforms');
    return result.data;
}

export { getPlatformServicePlatforms };