﻿#region LGPL Header
// Copyright (C) 2010, Jackie Ng
// http://code.google.com/p/fdotoolbox, jumpinjackie@gmail.com
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
//
// See license.txt for more/additional licensing information
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FdoToolbox.Base.Controls;
using FdoToolbox.Base.Services;
using FdoToolbox.Core.Feature;

namespace FdoToolbox.Express.Controls
{
    [ToolboxItem(false)]
    public partial class CreateRdbmsCtl : ViewContent, ICreateRdbmsView
    {
        private CreateRdbmsPresenter _presenter;

        public CreateRdbmsCtl()
        {
            InitializeComponent();
            _presenter = new CreateRdbmsPresenter(this, ServiceManager.Instance.GetService<IFdoConnectionManager>());
        }

        public virtual bool IsFdoMetadataOptional
        {
            get { return true; }
        }

        protected override void OnLoad(EventArgs e)
        {
            _presenter.Init();
            chkFdoMetadata.Enabled = this.IsFdoMetadataOptional;
        }

        public string Service
        {
            get { return txtService.Text; }
        }

        public string Username
        {
            get { return txtUsername.Text; }
        }

        public string Password
        {
            get { return txtPassword.Text; }
        }

        public bool ExtentsEnabled
        {
            set 
            {
                txtLowerLeftX.Enabled = 
                txtLowerLeftY.Enabled = 
                txtUpperRightX.Enabled = 
                txtUpperRightY.Enabled = value;
            }
        }

        public bool SubmitEnabled
        {
            set { btnOK.Enabled = value; }
        }

        public bool FdoMetadataEnabled
        {
            set { chkFdoMetadata.Enabled = value; }
        }

        public bool WKTEnabled
        {
            set { txtCSWkt.Enabled = value; }
        }

        public string DataStoreName
        {
            get { return txtName.Text; }
        }

        public string SchemaFile
        {
            get { return txtSchemaFile.Text; }
        }

        public OSGeo.FDO.Commands.SpatialContext.SpatialContextExtentType ExtentType
        {
            get { return (OSGeo.FDO.Commands.SpatialContext.SpatialContextExtentType)cmbExtentType.SelectedItem; }
        }

        public bool UseFdoMetadata
        {
            get { return chkFdoMetadata.Enabled && chkFdoMetadata.Checked; }
        }

        public bool ConnectOnCreate
        {
            get { return chkConnect.Checked; }
        }

        public bool FixSchema
        {
            get { return chkAlterSchema.Checked; }
        }

        public string CSName
        {
            get { return txtCSName.Text;  }
        }

        public string CSWkt
        {
            get { return txtCSWkt.Text; }
        }

        public double Tolerance
        {
            get { return double.Parse(txtTolerance.Text); }
        }

        public string ConnectionName
        {
            get { return txtConnectionName.Text; }
        }

        public virtual string Provider
        {
            get { throw new NotImplementedException(); }
        }

        public virtual string ServiceParameter
        {
            get { return "Service"; }
        }

        public virtual string UsernameParameter
        {
            get { return "Username"; }
        }

        public virtual string PasswordParameter
        {
            get { return "Password"; }
        }

        public virtual string DataStoreParameter
        {
            get { return "DataStore"; }
        }

        public virtual string FdoEnabledParameter
        {
            get
            {
                return "IsFdoEnabled";
            }
        }

        public double LowerLeftX
        {
            get { return double.Parse(txtLowerLeftX.Text); }
        }

        public double LowerLeftY
        {
            get { return double.Parse(txtLowerLeftY.Text); }
        }

        public double UpperRightX
        {
            get { return double.Parse(txtUpperRightX.Text); }
        }

        public double UpperRightY
        {
            get { return double.Parse(txtUpperRightY.Text); }
        }

        public Array AvailableExtentTypes
        {
            set { cmbExtentType.DataSource = value; }
        }

        private void cmbExtentType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtSchemaFile.Text = dlg.FileName;
                }
            }
        }

        public virtual SpatialContextInfo CreateDefaultSpatialContext()
        {
            var sc = new SpatialContextInfo();
            sc.Name = "Default";
            sc.XYTolerance = sc.ZTolerance = this.Tolerance;
            sc.CoordinateSystem = this.CSName;
            if (_presenter.RequiresWKT)
                sc.CoordinateSystemWkt = this.CSWkt;

            sc.IsActive = true;
            sc.ExtentType = this.ExtentType;
            if (sc.ExtentType == OSGeo.FDO.Commands.SpatialContext.SpatialContextExtentType.SpatialContextExtentType_Static)
            {
                string wktfmt = "POLYGON (({0} {1}, {2} {3}, {4} {5}, {6} {7}, {0} {1}))";
                double llx = Convert.ToDouble(this.LowerLeftX);
                double lly = Convert.ToDouble(this.LowerLeftY);
                double urx = Convert.ToDouble(this.UpperRightX);
                double ury = Convert.ToDouble(this.UpperRightY);
                sc.ExtentGeometryText = string.Format(wktfmt,
                    llx, lly,
                    urx, lly,
                    urx, ury,
                    llx, ury);
            }
            return sc;
        }

        private void txtConnectionName_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {

        }

        private void chkConnect_CheckedChanged(object sender, EventArgs e)
        {
            txtConnectionName.Enabled = chkConnect.Checked;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_presenter.Create())
            {
                ShowMessage("Create Data Store", "Data store created");
                this.Close();
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                if (_presenter.Test())
                {
                    this.ShowMessage("Test Connection", "Test connection succeeded");
                }
            }
            catch(Exception ex)
            {
                this.ShowError(ex);
            }
        }
    }
}
