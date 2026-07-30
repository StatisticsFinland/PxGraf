import { createTheme } from "@mui/material";

const palette = {
    primary: {
        main: '#1870C9',
        dark: '#072840',
        light: '#EDF4FB',
    },
    info: {
        main: '#0073b0',
    },
    background: {
        default: '#F5F7FA',
        paper: '#FFFFFF',
    },
    divider: '#dcdcdc',
    text: {
        primary: '#000000',
        secondary: '#666666',
        disabled: '#A8B2BF',
    },
        warning: {
        main: '#E06D10',
        dark: '#A66B00',
            light: '#FFF9E8',
        },
}

const theme = createTheme({
    palette: palette,
    shape: {
        borderRadius: 4,
    },
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
                        color: `${palette.text.secondary} !important`
                    },
                }
            }
        },
        MuiButton: {
            variants: [
                {
                    props: { variant: 'contained' },
                    style:{
                        color: palette.background.paper,
                        backgroundColor: palette.primary.main,
                        '&:hover': {
                            backgroundColor: palette.primary.dark,
                        },
                        '&:focus-visible': {
                            outline: `2px solid ${palette.text.primary}`,
                            backgroundColor: palette.primary.dark,
                        },
                        '&:active': {
                            backgroundColor: palette.primary.main,
                        }
                    }
                }
            ],
            defaultProps: {
                disableFocusRipple: true,

            },
            styleOverrides: {
                root: {
                    color: palette.primary.main,
                    minHeight: '36px',
                    fontWeight: 600,
                    letterSpacing: 0,
                    textTransform: 'none',
                    outlineColor: 'inherit',
                    '&:hover': {
                        backgroundColor: palette.primary.light,
                    },
                    '&:focus-visible': {
                        outline: `2px solid ${palette.primary.dark}`,
                        backgroundColor: palette.primary.light,
                    },
                    '&:active': {
                        color: palette.background.paper,
                        backgroundColor: palette.primary.main,
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
                        color: palette.text.primary,
                        border: '1px solid',
                        borderColor: palette.text.disabled,
                        outline: 'unset',
                    },
                    '& .Mui-selected': {
                        color: `${palette.primary.main} !important`,
                        backgroundColor: `${palette.primary.light} !important`,
                        borderColor: `${palette.primary.main} !important`,
                        boxShadow: `inset 0 0 0 2px ${palette.primary.main}`,
                        fontWeight: 600,
                        '& b': {
                            fontWeight: 'inherit',
                        },
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
                    minHeight: '36px',
                    padding: '6px 12px',
                    fontWeight: 500,
                    letterSpacing: 0,
                    lineHeight: 1.25,
                    textTransform: 'none',
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
                        color: palette.text.secondary,
                        outlineWidth: '2px',
                        outlineColor: 'inherit',
                        '&.Mui-checked': {
                            color: palette.primary.main,
                        }
                    }
                }
            }
        },
        MuiAlert: {
            styleOverrides: {
                standardWarning: {
                    '& .MuiAlert-icon': {
                        color: palette.warning.main
                    }
                }
            }
        },
        MuiCssBaseline: {
            styleOverrides: {
                body: {
                    fontFamily: [
                        '-apple-system',
                        'BlinkMacSystemFont',
                        '"Segoe UI"',
                        'Roboto',
                        'Oxygen',
                        'Ubuntu',
                        'Cantarell',
                        '"Fira Sans"',
                        '"Droid Sans"',
                        '"Helvetica Neue"',
                        'sans-serif',
                    ].join(', '),
                },
                '*:focus-visible': {
                    outline: `2px solid ${palette.primary.dark}`,
                },
                code: {
                    fontFamily: 'source-code-pro, Menlo, Monaco, Consolas, "Courier New", monospace',
                },
            },
        }
    },
    typography: {
        fontFamily: [
            '-apple-system',
            'BlinkMacSystemFont',
            '"Segoe UI"',
            'Roboto',
            'Oxygen',
            'Ubuntu',
            'Cantarell',
            '"Fira Sans"',
            '"Droid Sans"',
            '"Helvetica Neue"',
            'sans-serif',
        ].join(', '),
        fontWeightRegular: 400,
        fontWeightMedium: 500,
        fontWeightBold: 600,
        h1: {
            fontSize: '1.5rem',
            fontWeight: 600,
        },
        h2: {
            fontSize: '1rem',
            fontWeight: 600,
        }
    }
});

export default theme;