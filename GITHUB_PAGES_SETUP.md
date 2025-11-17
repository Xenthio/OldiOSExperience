# GitHub Pages Setup Instructions

This document provides step-by-step instructions for setting up GitHub Pages deployment for this repository.

## Quick Setup

### 1. Enable GitHub Pages with Actions

1. Go to your repository on GitHub: `https://github.com/Xenthio/OldiOSExperience`
2. Click on **Settings** (tab near the top)
3. In the left sidebar, click **Pages** (under "Code and automation")
4. Under **"Build and deployment"**, find the **"Source"** dropdown
5. Select **"GitHub Actions"** from the dropdown
6. That's it! No other configuration is needed.

### 2. Trigger the First Deployment

The workflow will automatically run when you merge this PR to the `main` branch. Alternatively, you can trigger it manually:

1. Go to the **Actions** tab in your repository
2. Click on the **"Deploy to GitHub Pages"** workflow in the left sidebar
3. Click the **"Run workflow"** button (on the right side)
4. Select `main` branch
5. Click **"Run workflow"**

### 3. Access Your Site

After the workflow completes (usually 2-5 minutes), your site will be available at:

**https://xenthio.github.io/OldiOSExperience/**

You can find the exact URL in:
- The Actions workflow summary page (under "Deploy to GitHub Pages" step)
- Settings → Pages → "Your site is live at..."

## Technical Details

### Deployment Configuration

- **Type**: Project site (compatible with your existing user GitHub Pages site)
- **URL Pattern**: `https://<username>.github.io/<repository-name>/`
- **Base Path**: `/OldiOSExperience/` (automatically configured in the workflow)
- **Trigger**: Automatic on push to `main` branch, or manual via Actions tab
- **Build**: Blazor WebAssembly in Release mode

### What the Workflow Does

1. Checks out the code
2. Sets up .NET 9.0 SDK
3. Restores NuGet packages
4. Builds the project in Release configuration
5. Publishes the Blazor WebAssembly app
6. Updates the base path for subdirectory deployment
7. Adds `.nojekyll` file (prevents Jekyll from processing the site)
8. Uploads the site as a GitHub Pages artifact
9. Deploys to GitHub Pages

### Workflow File

The workflow is defined in `.github/workflows/deploy-gh-pages.yml`

## Troubleshooting

### Workflow Fails

1. Check the Actions tab for error messages
2. Ensure .NET 9.0 SDK is available (it should be in the workflow)
3. Verify the project builds locally: `cd OldiOS/OldiOS.Web && dotnet build`

### Site Shows 404

1. Ensure GitHub Pages source is set to "GitHub Actions" (not a branch)
2. Wait a few minutes - deployment can take 2-5 minutes
3. Check the workflow completed successfully in the Actions tab
4. Verify you're using the correct URL with the base path

### Styles or Resources Not Loading

This is handled by the workflow:
- The base path is automatically updated from `/` to `/OldiOSExperience/`
- The `.nojekyll` file prevents GitHub Pages from processing assets with underscores

### Want to Use a Custom Domain?

1. In Settings → Pages, enter your custom domain in the "Custom domain" field
2. Update the workflow to change the base path from `/OldiOSExperience/` to `/` (or remove the sed command entirely)
3. Configure your DNS according to [GitHub's custom domain documentation](https://docs.github.com/en/pages/configuring-a-custom-domain-for-your-github-pages-site)

## Updating the Deployment

Any push to the `main` branch will automatically trigger a new deployment. The site will be updated within 2-5 minutes.

To deploy changes:
1. Make your changes
2. Commit and push to `main` (or merge a PR)
3. Wait for the workflow to complete
4. Your site will be updated automatically

## Additional Resources

- [GitHub Pages Documentation](https://docs.github.com/en/pages)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Blazor WebAssembly Hosting](https://docs.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/webassembly)
