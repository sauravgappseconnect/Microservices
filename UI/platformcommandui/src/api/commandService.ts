import axios from 'axios';
import  { type PlatformResponseSchema } from "../types/commandService";

const commandServiceUrl = import.meta.env.VITE_COMMAND_SERVICE_URL;

const requestInstance = axios.create({
    baseURL: `${commandServiceUrl}/api`,
    timeout: 5000,
});

const getCommandServicePlatforms = async function () {
    const result = await requestInstance.get<PlatformResponseSchema[]>('/platform/GetAllPlatforms');
    return result.data;
}

export { getCommandServicePlatforms };