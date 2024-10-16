import React from 'react';
import '@testing-library/jest-dom';
import { render, screen } from '@testing-library/react';
import { MultiselectableSelector } from './MultiselectableSelector';
import { IDimension, EDimensionType } from "types/cubeMeta";
import { IVisualizationSettings } from '../../../types/visualizationSettings';

const mockVariables: IDimension[] = [
	{
		Code: 'cVar',
		Name: { fi: 'cVarName' },
		Type: EDimensionType.Content,
		Values: [
			{
				Code: 'cVal',
				Name: { fi: 'cValName' },
				IsVirtual: false
			}
		]
	},
	{
		Code: 'tVar',
		Name: { fi: 'tVarName' },
		Type: EDimensionType.Time,
		Values: [
			{
				Code: 'tVal',
				Name: { fi: 'tValName' },
				IsVirtual: false
			}
		]
	},
	{
		Code: 'msVar',
		Name: { fi: 'msVarAName' },
		Type: EDimensionType.Other,
		Values: [
			{
				Code: 'msVal1',
				Name: { fi: 'msVal1Name' },
				IsVirtual: false
			},
			{
				Code: 'msVal2',
				Name: { fi: 'msVal2Name' },
				IsVirtual: false
			}
		]
	}
];

const mockSettingsChangedHandler = jest.fn();
const mockVisualizationSettings: IVisualizationSettings = {
	multiselectableVariableCode: 'msVar'
};
const mockVisualizationRules = {
	allowManualPivot: false, sortingOptions: null, multiselectVariableAllowed: true
};

jest.mock('react-i18next', () => ({
	...jest.requireActual('react-i18next'),
	useTranslation: () => {
		return {
			t: (str: string) => str,
			i18n: {
				changeLanguage: () => new Promise(() => null),
			},
		};
	},
}));

describe('Rendering test', () => {
	it('renders correctly', () => {
		const { asFragment } = render(
			<MultiselectableSelector
				visualizationRules={mockVisualizationRules}
				settingsChangedHandler={mockSettingsChangedHandler}
				visualizationSettings={mockVisualizationSettings}
				variables={mockVariables}
			/>);
		expect(asFragment()).toMatchSnapshot();
	});
});



describe('Assertion tests', () => {
	it('When no multiselect variable code is provided, the selector should default to "noMultiselectable"', () => {
		const mockSettingsChangedHandler = jest.fn();
		render(<MultiselectableSelector
			variables={mockVariables}
			visualizationRules={mockVisualizationRules}
			visualizationSettings={{ ...mockVisualizationSettings, multiselectableVariableCode: null }}
			settingsChangedHandler={mockSettingsChangedHandler}
		></MultiselectableSelector>);
		expect(screen.getByDisplayValue("noMultiselectable")).toBeInTheDocument();
	});

	it('When multiselect variable code is provided, the selector should be rendered with a corresponding value', () => {
		const mockSettingsChangedHandler = jest.fn();
		const changedMockSettings = { ...mockVisualizationSettings, multiselectableVariableCode: "msVar" }
		render(<MultiselectableSelector
			variables={mockVariables}
			visualizationRules={mockVisualizationRules}
			visualizationSettings={changedMockSettings}
			settingsChangedHandler={mockSettingsChangedHandler}
		></MultiselectableSelector>);
		expect(screen.queryByDisplayValue("msVar")).toBeInTheDocument();
	});
});