
import Button from '@mui/material/Button';
import { styled } from '@mui/material/styles';
import Dialog from '@mui/material/Dialog';
import DialogTitle from '@mui/material/DialogTitle';
import DialogContent from '@mui/material/DialogContent';
import DialogActions from '@mui/material/DialogActions';
import IconButton from '@mui/material/IconButton';
import CloseIcon from '@mui/icons-material/Close';
import type { PlatformResponseSchema } from '../../types/commandService';
import { useState } from 'react';
import { Box, TextField } from '@mui/material';
import { createPlatformService, updatePlatformService } from '../../api/platformService';
import { toast } from 'react-toastify';
import axios from 'axios';

const BootstrapDialog = styled(Dialog)(({ theme }) => ({
    '& .MuiDialogContent-root': {
        padding: theme.spacing(2),
    },
    '& .MuiDialogActions-root': {
        padding: theme.spacing(1),
    },
}));

type PlatformType = {
    setShowPlatformDetails: (value: PlatformResponseSchema | undefined) => void
    platformDetails: PlatformResponseSchema | undefined
}

export default function Platform({ platformDetails, setShowPlatformDetails }: PlatformType) {

    const [open, setOpen] = useState(true);
    const [isSaving, setIsSaving] = useState(false);

    const handleClose = () => {
        setOpen(false);
        setShowPlatformDetails(undefined);
    };

    const [formData, setFormData] = useState({
        id: platformDetails?.id,
        name: platformDetails?.name,
        publisher: platformDetails?.publisher,
        cost: platformDetails?.cost,
    } as PlatformResponseSchema);

    async function savePlatformDetails() {
        setIsSaving(true);
        try {
            if (formData.id) {
                await updatePlatformService(formData);
            }
            else {
                await createPlatformService(formData);
            }
        }
        catch (error) {
            if (axios.isAxiosError(error)) {
                toast.error(error.response?.data?.message ?? "Request failed");
            } else if (error instanceof Error) {
                toast.error(error.message);
            } else {
                toast.error("Unexpected error");
            }
        }
        finally {
            setIsSaving(false);
            handleClose();
        }
    }


    return (
        <>
            <BootstrapDialog
                onClose={handleClose}
                aria-labelledby="customized-dialog-title"
                open={open}
            >
                <DialogTitle sx={{ m: 0, p: 2 }} id="customized-dialog-title">
                    {formData.id ? formData.name : "Create new platform"}
                </DialogTitle>
                <IconButton
                    aria-label="close"
                    onClick={handleClose}
                    sx={(theme) => ({
                        position: 'absolute',
                        right: 8,
                        top: 8,
                        color: theme.palette.grey[500],
                    })}
                >
                    <CloseIcon />
                </IconButton>
                <DialogContent dividers>
                    <Box sx={{ display: "flex", flexDirection: "column", gap: 2, mt: 1 }}>
                        <TextField label="Name" value={formData.name}
                            onChange={(e) => { setFormData({ ...formData, name: e.target.value }) }}
                            fullWidth />
                        <TextField label="Publisher" value={formData.publisher}
                            onChange={(e) => { setFormData({ ...formData, publisher: e.target.value }) }}
                            fullWidth />
                        <TextField label="Cost" value={formData.cost}
                            type="number"
                            onChange={(e) => { setFormData({ ...formData, cost: Number(e.target.value) }) }}
                            fullWidth />
                    </Box>
                </DialogContent>
                <DialogActions>
                    <Button loading={isSaving} autoFocus onClick={savePlatformDetails}>
                        Save changes
                    </Button>
                </DialogActions>
            </BootstrapDialog>
        </>
    );
}
