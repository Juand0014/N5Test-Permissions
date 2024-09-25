import { PermissionType } from ".";

export interface Permission {
	id: string | null;
	nombreEmpleado: string;
	apellidoEmpleado: string;
	fechaPermiso: string;
	tipoPermiso?: number | null;
	permissionType?: PermissionType;
}