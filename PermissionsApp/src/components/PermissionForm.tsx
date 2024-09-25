import { Button, Container, TextField } from "@mui/material";
import React, { useState } from "react";
import { Permission, PermissionType } from "../models";
import { http } from "../services";
import { v4 as uuid } from "uuid";

const PermissionForm: React.FC = () => {
    const [permissionData, setPermissionData] = useState<Permission>({
        id: uuid(),
        nombreEmpleado: "",
        apellidoEmpleado: "",
        fechaPermiso: "",
        tipoPermiso: null,
        permissionType: {
            id: 0,
            description: null,
        },
    });

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;

        setPermissionData((prevData) => {
            if (name === "description") {
                return {
                    ...prevData,
                    permissionType: {
                        ...prevData.permissionType,
                        description: value,
                    } as PermissionType,
                };
            }
            return { ...prevData, [name]: value };
        });
    };

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        try {
            const permissionPayload = {
                ...permissionData,
                tipoPermiso: permissionData.tipoPermiso
                    ? Number(permissionData.tipoPermiso)
                    : undefined,
                permissionType: permissionData.permissionType?.description
                    ? permissionData.permissionType
                    : undefined,
            };

            await http.post("/Permission", permissionPayload);
            console.log("Permiso creado con éxito");
        } catch (error) {
            console.error("Error al crear el permiso:", error);
        }
    };

    const showTipoPermisoInput = !permissionData.permissionType?.description;
    const showDescriptionInput = !permissionData.tipoPermiso;

    return (
        <Container>
            <form onSubmit={handleSubmit}>
                <TextField
                    label="Nombre"
                    name="nombreEmpleado"
                    value={permissionData.nombreEmpleado}
                    onChange={handleChange}
                    variant="outlined"
                    fullWidth
                    margin="normal"
                    required
                />

                <TextField
                    label="Apellido"
                    name="apellidoEmpleado"
                    value={permissionData.apellidoEmpleado}
                    onChange={handleChange}
                    variant="outlined"
                    fullWidth
                    margin="normal"
                    required
                />

                <TextField
                    label="Fecha"
                    name="fechaPermiso"
                    value={permissionData.fechaPermiso}
                    onChange={handleChange}
                    type="date"
                    variant="outlined"
                    fullWidth
                    margin="normal"
                    required
                />

                {showTipoPermisoInput && (
                    <TextField
                        label="Tipo de Permiso"
                        name="tipoPermiso"
                        value={permissionData.tipoPermiso || ""}
                        onChange={handleChange}
                        variant="outlined"
                        fullWidth
                        margin="normal"
                        type="number"
                    />
                )}

                {showDescriptionInput && (
                    <TextField
                        label="Nuevo tipo de permiso"
                        name="description"
                        value={permissionData.permissionType?.description || ""}
                        onChange={handleChange}
                        variant="outlined"
                        fullWidth
                        margin="normal"
                    />
                )}

                <Button type="submit" variant="contained" color="primary">
                    Crear Permiso
                </Button>
            </form>
        </Container>
    );
};

export default PermissionForm;
