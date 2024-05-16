import { createTheme } from "@mui/material";

const palette = {
    primary: {
        main: '#1A56EC',
        dark: '#1A3061',
        light: '#E8EEFD',  
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
                        color: 'var(--text-gray) !important'
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
                        backgroundColor: 'var(--selected-blue)',
                        '&:hover': {
                            backgroundColor: 'var(--selected-blue-dark)',
                        },
                        '&:focus-visible': {
                            outline: '2px solid black',
                            backgroundColor: 'var(--selected-blue-dark)',
                        },
                        '&:active': {
                            backgroundColor: 'var(--selected-blue)',
                        }
                    }
                }
            ],
            defaultProps: {
                disableFocusRipple: true,

            },
            styleOverrides: {
                root: {
                    color: 'var(--selected-blue)',
                    outlineColor: 'inherit',
                    '&:hover': {
                        backgroundColor: 'var(--selected-blue-light)',
                    },
                    '&:focus-visible': {
                        outline: `2px solid ${palette.primary.dark}`,
                        backgroundColor: 'var(--selected-blue-light)',
                    },
                    '&:active': {
                        color: 'white',
                        backgroundColor: 'var(--selected-blue)',
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
                        color: 'var(--text-gray)',
                        border: '1px solid',
                        borderColor: 'inherit',
                        outline: 'unset',
                    },
                    '& .Mui-selected': {
                        color: 'var(--selected-blue) !important',
                        backgroundColor: 'var(--selected-blue-light) !important',
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
                        color: 'var(--text-gray)',
                        outlineWidth: '2px',
                        outlineColor: 'inherit',
                        '&.Mui-checked': {
                            color: 'var(--selected-blue)',
                        }
                    }
                }
            }
        },
        MuiAlert: {
            styleOverrides: {
                standardWarning: {
                    '& .MuiAlert-icon': {
                        color: 'var(--warning-orange)'
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