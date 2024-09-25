import { AxiosResponse } from 'axios';
import { api } from '../config';
import { Permission } from '../models';

const get = async (endpoint: string): Promise<AxiosResponse<Permission[]>> => {
	return (await api.get(endpoint)).data;
};

const post = async (endpoint: string, data: Permission): Promise<AxiosResponse<void>> => {
	return (await api.post(endpoint, data)).data;
};

const update = async (endpoint: string, data: Permission): Promise<AxiosResponse<void>> => {
	return (await api.put(endpoint, data)).data;
}

export const http = {
	get,
	post,
	update
} 