import { createTheme } from "@mui/material";

const palette = {
    primary: {
        main: '#1A56EC',
        dark: '#1A3061',
        light: '#E8EEFD',  
    },
    custom: {
        tableHover: '#d1ddfb',
        textGray: '#666666',
        textBlack: '#000000',
        selectedBlue: '#1870C9',
        selectedBlueLight: '#EDF4FB',
        selectedBlueDark: '#072840',
        editorfieldOutline: '#839583',
        editorfieldBackground: '#EEFFEE',
        editorfieldOutlineEdited: '#949400',
        editorfieldBackgroundEdited: '#FFFFDD',
        warningOrange: '#E06D10',
        surfaceLight: '#f8f8f8',
        surfaceWhite: '#ffffff',
        borderLight: '#dcdcdc',
        textMuted: '#cccccc',
        infoBlue: '#0073b0',
        shadowColor: '#737373',
    }
}

const theme = createTheme({
    palette: palette,
    components: {
        MuiAccordionSummary: {
            styleOverrides: {
                root: {
                    '&:focus-visible': {
                        outline: `2px solid ${palette.primary.dark}`,
                        outlineOffset: '-2px',
                        backgroundColor: 'unset',
                    }
                }
            }
        },
        MuiAutocomplete: {
            defaultProps: {
                clearOnEscape: true,
            },
            styleOverrides: {
                root: {
                    '& .MuiChip-deleteIcon': {
                        color: `${palette.custom.textGray} !important`
                    },
                }
            }
        },
        MuiButton: {
            variants: [
                {
                    props: { variant: 'contained' },
                    style:{
                        color: 'white',
                        backgroundColor: palette.custom.selectedBlue,
                        '&:hover': {
                            backgroundColor: palette.custom.selectedBlueDark,
                        },
                        '&:focus-visible': {
                            outline: '2px solid black',
                            backgroundColor: palette.custom.selectedBlueDark,
                        },
                        '&:active': {
                            backgroundColor: palette.custom.selectedBlue,
                        }
                    }
                }
            ],
            defaultProps: {
                disableFocusRipple: true,

            },
            styleOverrides: {
                root: {
                    color: palette.custom.selectedBlue,
                    outlineColor: 'inherit',
                    '&:hover': {
                        backgroundColor: palette.custom.selectedBlueLight,
                    },
                    '&:focus-visible': {
                        outline: `2px solid ${palette.primary.dark}`,
                        backgroundColor: palette.custom.selectedBlueLight,
                    },
                    '&:active': {
                        color: 'white',
                        backgroundColor: palette.custom.selectedBlue,
                    }
                }
            }
        },
        MuiListItemButton: {
            styleOverrides: {
                root: {
                    '&:focus-visible': {
                        outline: `2px solid ${palette.primary.dark}`,
                        backgroundColor: 'transparent',
                        outlineOffset: '-2px',
                    },
                }
            }
        },
        MuiIconButton: {
            defaultProps: {
                disableFocusRipple: true,
            },
            styleOverrides: {
                root: {
                    '&:focus-visible': {
                        outline: `2px solid ${palette.primary.dark}`,
                    }
                }
            }
        },
        MuiTab: {
            defaultProps: {
                disableFocusRipple: true,
                tabIndex: 0,
            },
            styleOverrides: {
                root: {
                    '&:focus-visible': {
                        outline: `2px solid ${palette.primary.dark}`,
                        outlineOffset: '-2px',
                    }
                }
            }
        },
        MuiToggleButtonGroup: {
            styleOverrides: {
                root: {
                    '& .MuiToggleButton-root': {
                        color: palette.custom.textGray,
                        border: '1px solid',
                        borderColor: 'inherit',
                        outline: 'unset',
                    },
                    '& .Mui-selected': {
                        color: `${palette.custom.selectedBlue} !important`,
                        backgroundColor: `${palette.custom.selectedBlueLight} !important`,
                        outline: '2px solid',
                        margin: '2px',
                    },

                }
            }
        },
        MuiToggleButton: {
            defaultProps: {
                disableFocusRipple: true,
                tabIndex: 0,
            },
            styleOverrides: {
                root: {
                    '&:focus-visible': {
                        outline: `2px solid ${palette.primary.dark} !important`,
                        outlineOffset: '2px',
                    }
                }
            }
        },
        MuiSwitch: {
            styleOverrides: {
                root: {
                    '& .MuiSwitch-switchBase': {
                        color: palette.custom.textGray,
                        outlineWidth: '2px',
                        outlineColor: 'inherit',
                        '&.Mui-checked': {
                            color: palette.custom.selectedBlue,
                        }
                    }
                }
            }
        },
        MuiAlert: {
            styleOverrides: {
                standardWarning: {
                    '& .MuiAlert-icon': {
                        color: palette.custom.warningOrange
                    }
                }
            }
        }
    },
    typography: {
        h1: {
            fontSize: '1.5rem',
        },
        h2: {
            fontSize: '1rem'
        }
    }
});

export default theme;