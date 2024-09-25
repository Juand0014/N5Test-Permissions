import { Button, Container, TextField } from "@mui/material";
import React, { useState } from "react";
import { Permission, PermissionType } from "../models";
import { http } from "../services/services";

const PermissionForm: React.FC = () => {

	const [ permissionData, setPermission ] = useState<Permission>({
		id:0,
		nombreEmpleado:'',
		apellidoEmpleado:'',
		fechaPermiso:'',
		tipoPermiso:0,
		permissionType: {
			id: 0,
			description: null
		}
	})

	const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		const { name, value } = e.target;
		if (name === 'description') {
			setPermission((prevData) => ({
				...prevData,
				permissionType: {
					...prevData.permissionType,
					description: value, 
				} as PermissionType,
			}));
		}else{
			setPermission({
				...permissionData,
				[name]: value
			})
		}
	}

	const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
		e.preventDefault();
		try{
			permissionData.tipoPermiso = Number(permissionData.tipoPermiso);
			if(permissionData.permissionType?.description == null) delete permissionData.permissionType;
			else permissionData.permissionType.id = permissionData.tipoPermiso;

			await http.post('/Permission', permissionData);

		}catch(error){
			console.log(error);
		}
	}

	return (
		<Container>
			<form onSubmit={handleSubmit}>
				<TextField
					label="Name"
					name="nombreEmpleado"
					value={permissionData.nombreEmpleado || ''}
					onChange={handleChange}
					variant="outlined"
					fullWidth
					margin="normal"
					required
				/>

				<TextField
					label="Apellido"
					name="apellidoEmpleado"
					value={permissionData.apellidoEmpleado || ''}
					onChange={handleChange}
					variant="outlined"
					fullWidth
					margin="normal"
					required
				/>

				<TextField
					label="Fecha"
					name="fechaPermiso"
					value={permissionData.fechaPermiso || ''}
					onChange={handleChange}
					type="date"
					variant="outlined"
					fullWidth
					margin="normal"
					required
				/>

				<TextField
					label="Tipo de Permiso"
					name="tipoPermiso"
					value={permissionData.tipoPermiso}
					onChange={handleChange}
					variant="outlined"
					fullWidth
					margin="normal"
					type="number"
				/>

				<TextField
					label="Nuevo tipo de permiso"
					name="description"
					value={permissionData.permissionType?.description}
					onChange={handleChange}
					variant="outlined"
					fullWidth
					margin="normal"
				/>

				<Button type="submit" variant="contained" color="primary">
					Crear Permiso
				</Button>
					
			</form>
		</Container>
	);
}

export default PermissionForm;