import { PermissionType } from ".";

export interface Permission {
	id: number;
	nombreEmpleado: string;
	apellidoEmpleado: string;
	fechaPermiso: string;
	tipoPermiso: number;
	permissionType?: PermissionType;
}